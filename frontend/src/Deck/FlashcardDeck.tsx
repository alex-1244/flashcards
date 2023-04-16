import React, { useState, useEffect } from "react";
import Flashcard, { FlashcardProps } from "../Flashcard/Flashcard";

const FlashcardDeck: React.FC<{ flashcards: FlashcardProps[] }> = ({ flashcards }) => {
  const [currentIndex, setCurrentIndex] = useState<number>(0);
  const [shuffledCards, setShuffledCards] = useState<FlashcardProps[]>([]);

  useEffect(() => {
    setShuffledCards(shuffle(flashcards));
  }, [flashcards]);

  const shuffle = (cards: FlashcardProps[]): FlashcardProps[] => {
    const shuffled = [...cards];
    for (let i = shuffled.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [shuffled[i], shuffled[j]] = [shuffled[j], shuffled[i]];
    }
    return shuffled;
  };

  const handleNextCard = (): void => {
    setCurrentIndex(currentIndex === shuffledCards.length - 1 ? 0 : currentIndex + 1);
  };

  return (
    <div>
      {shuffledCards[currentIndex] &&
       <Flashcard word={shuffledCards[currentIndex].word} definition={shuffledCards[currentIndex].definition} /> }
       <div className="next" onClick={handleNextCard}>NEXT</div>
    </div>
  );
};

export default FlashcardDeck;
