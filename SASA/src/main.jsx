import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { PrimeReactProvider } from 'primereact/api';
import 'primereact/resources/primereact.css';
import 'primereact/resources/themes/tailwind-light/theme.css';
import './index.css'
import App from './App.jsx'

//Prime react
import 'primereact/resources/primereact.min.css';

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <PrimeReactProvider>
    <App />
    </PrimeReactProvider>
  </StrictMode>,
)
