import { BrowserRouter, Route, Routes } from "react-router-dom";
import Layout from "./components/Layout/Layout";
import ProductDetailPage from "./pages/ProductDetailPage";
import ProductListPage from "./pages/ProductListPage";
import CartPage from "./pages/CartPage/CartPage"; // Add the extra /CartPage
import { CartProvider } from "./contexts/CartContext";
import styles from "./Layout.module.css";

export default function App() {
  return (
    <CartProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index element={<ProductListPage />} />
            <Route path="products/:id" element={<ProductDetailPage />} />
            <Route path="cart" element={<CartPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </CartProvider>
  );
}


import { CartBadge } from "./components/CartBadge/CartBadge";

// Inside your return JSX, make sure the component is actually called:
<header className={styles.header}>
  <h1>Buckeye Marketplace</h1>
  <CartBadge />  {/* <--- Is this line missing? */}
</header>