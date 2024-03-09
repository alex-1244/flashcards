import React, { useEffect, useState } from "react";
import apiConfig from "../../apiConfig";
import axios from "axios";
import TokenCache from "../TokenCache";
import { redirect, useParams, useSearchParams } from "react-router-dom";
import "./Partners.css";
import moment from "moment";
import { toBackendFormat, standardFormat } from "../../helpers/dateFormat";

const PartnerDetails: React.FC<any> = () => {
  const { id } = useParams();
  const [ getParams, setParams ] = useSearchParams();
  const [sales, setSales] = useState<any[]>([]);
  const [partnerName, setPartnerName] = useState<string>("");
  const [total, setTotal] = useState<any>({
    buy: 0,
    sale: 0,
    income: 0,
    amount: 0,
  });
  const [buttonDisabled, setButtonDisabled] = useState<boolean>(false);
  const [dateFrom, setDateFrom] = useState<string>(getParams.get("dateFrom") || "");
  const [dateTo, setDateTo] = useState<string>(getParams.get("dateTo") || "");
  const [partnerEmail, setPartnerEmail] = useState<string>(
    TokenCache.getEmail(+id!) ?? ""
  );

  let email: React.RefObject<HTMLInputElement> = React.createRef();

  useEffect(() => {
    loadSales(dateFrom, dateTo);
  }, []);

  async function loadSales(dateFrom: string, dateTo: string) {
    const data = await fetchSales(dateFrom, dateTo);
    setSales(data.products);
    setPartnerName(data.partnerName);
    setTotal(calculateToals(data.products));
    //setDateFrom(data.startDate.substring(0, 10));
    //setDateTo(data.endDate.substring(0, 10));
  }

  function calculateToals(products: any[]) {
    let total_buy = 0;
    let total_sale = 0;
    let total_income = 0;
    let total_amount = 0;

    products.map((p) => {
      total_buy += p.total_purchase;
      total_sale += p.total_price;
      total_income += p.total_margin_amount;
      total_amount += p.salesCount;
    });

    return {
      buy: total_buy,
      sale: total_sale,
      income: total_income,
      amount: total_amount,
    };
  }

  async function fetchSales(dateFrom: string, dateTo: string): Promise<any> {
    var partners = await axios.get(
      `${apiConfig.baseUrl}/keycrm/products?category=${id}&startDate=${toBackendFormat(dateFrom)}&endDate=${toBackendFormat(dateTo)}`,
      {
        headers: {
          Authentication: TokenCache.getToken(),
        },
      }
    );

    const data = await partners.data;
    return data;
  }

  const sendEmail = async () => {
    setButtonDisabled(true);
    setTimeout(() => {
      setButtonDisabled(false);
    }, 10000);

    TokenCache.setEmail(+id!, email.current!.value);

    const resp = await axios.post(
      `${apiConfig.baseUrl}/keycrm/report`,
      {
        Category: id,
        Email: email.current!.value,
        StartDate: toBackendFormat(dateFrom),
        EndDate: toBackendFormat(dateTo)
      },
      {
        headers: {
          Authentication: TokenCache.getToken(),
        },
      }
    );
  };

  const downloadReport = async () => {
    const resp = await axios({
      url:`${apiConfig.baseUrl}/keycrm/report-file?category=${id}&startDate=${toBackendFormat(dateFrom)}&endDate=${toBackendFormat(dateTo)}`,
      responseType: 'blob',
      headers:{
        Authentication: TokenCache.getToken()
      },
    }).then((response) => {
      // create file link in browser's memory
      const href = URL.createObjectURL(response.data);
  
      // create "a" HTML element with href to file & click
      const link = document.createElement('a');
      link.href = href;
      link.setAttribute('download', `${partnerName}.csv`); //or any other extension
      document.body.appendChild(link);
      link.click();
  
      // clean up "a" element & remove ObjectURL
      document.body.removeChild(link);
      URL.revokeObjectURL(href);
    });;
  };

  const changeDateFrom = async (e: any) => {
    const date = moment(e.target.value).format(standardFormat);
    setDateFrom(date);
    getParams.set("dateFrom", date);
    setParams(getParams);
    await loadSales(date, dateTo);
  };

  const changeDateTo = async (e: any) => {
    const date = moment(e.target.value).format(standardFormat);
    setDateTo(date);
    getParams.set("dateTo", date);
    setParams(getParams);
    await loadSales(dateFrom, date);
  };

  return (
    <>
      <div>{partnerName}</div>
      <div>
        <input aria-label="Початок періоду" value={dateFrom} onChange={changeDateFrom} type="date" />
        <span style={{"margin":"0 15px"}}>|</span>
        <input aria-label="Кінець періоду" value={dateTo} onChange={changeDateTo} type="date" />
      </div>
      <br/>
      <div>
        <table className="sales-table">
          <thead>
            <tr>
              <th>Назва товару</th>
              <th>Середня ціна</th>
              <th>Сума закупівлі</th>
              <th>Суума продажів</th>
              <th>Прибуток</th>
              <th>Кількість товарів</th>
            </tr>
          </thead>
          <tbody>
            {sales.map((sale) => (
              <tr key={sale.id}>
                <td>{sale.title}</td>
                <td>{sale.avg_price}</td>

                <td>{sale.total_purchase}</td>
                <td>{sale.total_price}</td>
                <td>{sale.total_margin_amount}</td>
                <td>{sale.salesCount}</td>
              </tr>
            ))}
            <tr key="total" className="total-row">
              <td>Разом</td>
              <td>-</td>
              <td>{total.buy}</td>
              <td>{total.sale}</td>
              <td>{total.income}</td>
              <td>{total.amount}</td>
            </tr>
          </tbody>
        </table>
        <div className="send-report-block">
          <input
            type="text"
            placeholder="Імейл бренда"
            ref={email}
            value={partnerEmail}
            onChange={(e) => setPartnerEmail(e.target.value)}
          ></input>
          <br />
          <button onClick={sendEmail} disabled={buttonDisabled}>
            Відправити звіт
          </button>
          <button onClick={downloadReport}>
            Скачати звіт
          </button>
        </div>
      </div>
    </>
  );
};

export default PartnerDetails;
