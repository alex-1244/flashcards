import React, { useEffect, useState } from "react";
import { FlashcardProps } from "../Flashcard/Flashcard"
import { Outlet, Link } from "react-router-dom";
import apiConfig from "../apiConfig";


export interface BinCard {
    name: string;
    binId: string;
  }

const Frontpage: React.FC = () => {
    const [bins, setBins] = useState<BinCard[]>([]);

    useEffect(() => {
      async function loadBins() {
        const data = await fetchBins();
        setBins(data);
      }
  
      loadBins();
    }, []);
  
    async function fetchBins(): Promise<BinCard[]> {
      const response = await fetch(`${apiConfig.baseUrl}/Frontpage`);
      const data = await response.json();
      return data as BinCard[];
    }


    return (
      <div className="flashcard">
       {bins && bins.map((bin: BinCard)=>
            <div style={{marginTop: "10px"}} key={bin.binId}>
                <Link to={`/${bin.binId}`}>{bin.name}</Link>
            </div>
        )}
      </div>
    );
  };
  
  export default Frontpage;

