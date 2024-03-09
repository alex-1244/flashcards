import React, { useEffect, useState } from "react";
import apiConfig from "../../apiConfig";
import TokenCache from "../TokenCache";
import { useNavigate, useSearchParams } from "react-router-dom";
import "./Partners.css";
import moment from "moment";
import axios from "axios";
import { standardFormat } from "../../helpers/dateFormat";"../../helpers/dateFormat"

const Partners: React.FC<any> = () => {
  const today = moment().format(standardFormat)
  
  const [ getParams, setParams ] = useSearchParams();
  const [partners, setPartners] = useState<any[]>([]);
  const [dateFrom, setDateFrom] = useState<string>(getParams.get('dateFrom') || today);
  const [dateTo, setDateTo] = useState<string>(getParams.get('dateTo') || today);
  const navigate = useNavigate();

  useEffect(() => {
    async function loadPartners() {
      const data = await fetchPartners();
      setPartners(data);
    }

    loadPartners();
  }, []);

  async function fetchPartners(): Promise<any[]> {
    var partners = await axios.get(`${apiConfig.baseUrl}/keycrm/partners`, {
      headers: {
        Authentication: TokenCache.getToken(),
      },
    });

    const data = await partners.data;
    return data.partners;
  }

  const goToPartner = (partner: any) => {
    navigate(`/ngd/partners/${partner.id}?dateFrom=${dateFrom}&dateTo=${dateTo}`);
  };

  const changeDateFrom = async (e: any) => {
    const date = moment(e.target.value).format(standardFormat);
    setDateFrom(date);
    getParams.set("dateFrom", date);
    setParams(getParams);
  };

  const changeDateTo = async (e: any) => {
    const date = moment(e.target.value).format(standardFormat);
    setDateTo(date);
    getParams.set("dateTo", date);
    setParams(getParams);
  };

  return (
    <>
      <div>
        <input aria-label="Початок періоду" value={dateFrom} onChange={changeDateFrom} type="date" />
        <span style={{"margin":"0 15px"}}>|</span>
        <input aria-label="Кінець періоду" value={dateTo} onChange={changeDateTo} type="date" />
      </div>
      {partners.map((p) => (
        <div className="flashcard" key={p.id} onClick={() => goToPartner(p)}>
          {p.name}
        </div>
      ))}
    </>
  );
};

export default Partners;
