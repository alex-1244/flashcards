import React, { useEffect, useState } from "react";
import apiConfig from "../../apiConfig";
import axios from "axios";
import TokenCache from "../TokenCache";
import { redirect, useNavigate } from "react-router-dom";
import "./Partners.css";

const Partners: React.FC<any> = () => {
  const [partners, setPartners] = useState<any[]>([]);
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
    console.log(data);
    return data.partners;
  }

  const goToPartner = (partner: any) => {
    console.log("clicked");
    navigate(`/ngd/partners/${partner.id}`);
  };

  return (
    <>
      {partners.map((p) => (
        <div className="flashcard" key={p.id} onClick={() => goToPartner(p)}>
          {p.name}
        </div>
      ))}
    </>
  );
};

export default Partners;
