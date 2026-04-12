import { BrowserRouter, Route, Routes, Link } from "react-router-dom";
import Layout from "./components/Layout/Layout";
import ProductDetailPage from "./pages/ProductDetailPage";
import ProductListPage from "./pages/ProductListPage";
import CartPage from "./pages/CartPage/CartPage";
import { CartProvider } from "./contexts/CartContext";
import { CartBadge } from "./components/CartBadge/CartBadge";

export default function App() {
  return (
    <CartProvider>
      <BrowserRouter>
        <div style={{ padding: '20px' }}>
          <header style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '40px' }}>
            <Link to="/" style={{ textDecoration: 'none', color: 'inherit' }}>
              <h1>Buckeye Marketplace</h1>
            </Link>
            <CartBadge />
          </header>

          <Routes>
            <Route path="/" element={<Layout />}>
              <Route index element={<ProductListPage />} />
              <Route path="products/:id" element={<ProductDetailPage />} />
              <Route path="cart" element={<CartPage />} />
            </Route>
          </Routes>
        </div>
      </BrowserRouter>
    </CartProvider>
  );
}