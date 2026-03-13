import { Link, Outlet } from "react-router-dom";
import { CartBadge } from "../CartBadge/CartBadge"; 
import styles from "./Layout.module.css";

export default function Layout() {
  return (
    <div className={styles.layout}>
      <header className={styles.header}>
        <div className={styles.headerInner}>
          <Link to="/" className={styles.logo}>
            <span className={styles.logoIcon}>🌰</span>
            <h1 className={styles.title}>Buckeye Marketplace</h1>
          </Link>
          
          <nav className={styles.nav}>
            <CartBadge />
          </nav>
        </div>
      </header>
      <main className={styles.main}>
        {/* Page content (ProductList, CartPage, etc.) renders here */}
        <Outlet />
      </main>
    </div>
  );
}