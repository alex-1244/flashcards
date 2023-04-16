import React, { useState } from "react";
import './Flashcard.css'

export interface FlashcardProps {
  word: string;
  definition: string;
}

const Flashcard: React.FC<FlashcardProps> = ({ word, definition }) => {
  const [isFlipped, setIsFlipped] = useState<boolean>(false);

  const flipCard = () => {
    setIsFlipped(!isFlipped);
  };

  return (
    <div className="flashcard" onClick={flipCard}>
      <div className={`card ${isFlipped ? "flipped" : ""}`}>
        <div className="front">
          <h3>{word}</h3>
        </div>
        <div className="back">
          <p>{definition}</p>
        </div>
      </div>
    </div>
  );
};

export default Flashcard;
