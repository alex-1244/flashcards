import React, { useEffect, useState } from "react";
import apiConfig from "../../apiConfig";
import axios from "axios";
import TokenCache from "../TokenCache";
import { redirect, useParams } from "react-router-dom";
import "./Partners.css";

const PartnerDetails: React.FC<any> = () => {
  const { id } = useParams();
  const [sales, setSales] = useState<any[]>([]);
  const [partnerName, setPartnerName] = useState<string>("");
  const [total, setTotal] = useState<any>({
    buy: 0,
    sale: 0,
    income: 0,
    amount: 0,
  });
  const [buttonDisabled, setButtonDisabled] = useState<boolean>(false);
  const [dateFrom, setDateFrom] = useState<string>("");
  const [dateTo, setDateTo] = useState<string>("");
  const [partnerEmail, setPartnerEmail] = useState<string>(
    TokenCache.getEmail(+id!) ?? ""
  );

  let email: React.RefObject<HTMLInputElement> = React.createRef();

  useEffect(() => {
    async function loadSales() {
      const data = await fetchSales();
      setSales(data.products);
      setPartnerName(data.partnerName);
      setTotal(calculateToals(data.products));
      setDateFrom(data.startDate.substring(0, 10));
      setDateTo(data.endDate.substring(0, 10));
    }

    loadSales();
  }, []);

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

  async function fetchSales(): Promise<any> {
    var partners = await axios.get(
      `${apiConfig.baseUrl}/keycrm/products?category=${id}`,
      {
        headers: {
          Authentication: TokenCache.getToken(),
        },
      }
    );

    const data = await partners.data;
    console.log(data);
    return data;
  }

  const sendEmail = async () => {
    setButtonDisabled(true);
    setTimeout(() => {
      setButtonDisabled(false);
    }, 10000);

    TokenCache.setEmail(+id!, email.current!.value);
    console.log(email.current!.value);

    const resp = await axios.post(
      `${apiConfig.baseUrl}/keycrm/report`,
      {
        Category: id,
        Email: email.current!.value,
      },
      {
        headers: {
          Authentication: TokenCache.getToken(),
        },
      }
    );

    console.log(resp);
  };

  return (
    <>
      <div>{partnerName}</div>
      <div>
        <span>{dateFrom}</span> - <span>{dateTo}</span>
      </div>
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
        </div>
      </div>
    </>
  );
};

export default PartnerDetails;
