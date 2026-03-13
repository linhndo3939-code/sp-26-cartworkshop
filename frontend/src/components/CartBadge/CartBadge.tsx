import { Link } from "react-router-dom";
// CHANGE: Import from your new hooks file, not the context file
import { useCartContext } from "../../hooks/UseCartContext"; 
import styles from "./CartBadge.module.css";

export function CartBadge() {
  const { cartItemCount } = useCartContext();

  return (
    <Link to="/cart" className={styles.cartButton} aria-label={`Shopping cart with ${cartItemCount} items`}>
      <span role="img" aria-hidden="true">🛒</span>
      {cartItemCount > 0 && (
        <span className={styles.badge}>
          {cartItemCount}
        </span>
      )}
    </Link>
  );
}