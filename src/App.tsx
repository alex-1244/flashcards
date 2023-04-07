import { useEffect, useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import FlashcardDeck from "./Deck/FlashcardDeck";
import { FlashcardProps } from "./Flashcard/Flashcard";

function App() {
  const [cards, setCards] = useState<FlashcardProps[]>([]);

  useEffect(() => {
    async function loadFlashcards() {
      const data = await fetchFlashcards();
      setCards(data);
    }

    loadFlashcards();
  }, []);

  async function fetchFlashcards(): Promise<FlashcardProps[]> {
    const response = await fetch("https://api.jsonbin.io/v3/b/642fbcfec0e7653a059f5b80/latest");
    const data = await response.json();
    return data.record as FlashcardProps[];
  }

  return (
    <div className="App">
      <FlashcardDeck flashcards={cards} />
    </div>
  )
}

export default App
