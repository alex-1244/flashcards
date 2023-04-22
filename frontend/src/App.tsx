import { useEffect, useState } from 'react'
import './App.css'
import FlashcardDeck from "./Deck/FlashcardDeck";
import { FlashcardProps } from "./Flashcard/Flashcard";
import { useParams } from 'react-router-dom';
import apiConfig from './apiConfig';

const App: React.FC = () => {

  const {binId} =  useParams();
  const [cards, setCards] = useState<FlashcardProps[]>([]);

  useEffect(() => {
    async function loadFlashcards() {
      const data = await fetchFlashcards();
      setCards(data);
    }

    loadFlashcards();
  }, []);

  async function fetchFlashcards(): Promise<FlashcardProps[]> {
    const response = await fetch(`${apiConfig.baseUrl}/${binId}`);
    const data = await response.json();
    console.log(data);
    return data as FlashcardProps[];
  }

  return (
    <div className="App">
      <FlashcardDeck flashcards={cards} />
    </div>
  )
}

export default App
