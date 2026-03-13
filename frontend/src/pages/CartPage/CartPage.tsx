import { Link } from "react-router-dom";
// 1. Up to CartPage folder, 2. Up to pages, 3. Up to src, then down to components/hooks
import { useCartContext } from "../../components/hooks/UseCartContext"; 

import { CheckoutForm } from "../../components/CheckoutForm/CheckoutForm";
import type { CartItem } from "../../types/cart"; 
import styles from "./CartPage.module.css";

export default function CartPage() {
  const { items, cartTotal, dispatch } = useCartContext();

  // 1. Handle Empty State
  if (items.length === 0) {
    return (
      <div className={styles.emptyContainer}>
        <h2>Your cart is empty</h2>
        <p>Add some Buckeye gear to get started!</p>
        <Link to="/" className={styles.backLink}> 
          Back to Products
        </Link>
      </div>
    );
  }

  // 2. Handle Cart with Items
  return (
    <div className={styles.cartContainer}>
      <h1 className={styles.title}>Your Shopping Cart</h1>
      
      <div className={styles.itemList}>
        {/* Added : CartItem here to fix the "any" type error */}
        {items.map((item: CartItem) => (
          <div key={item.product.id} className={styles.cartItem}>
            <div className={styles.itemInfo}>
              <h3>{item.product.name}</h3>
              <p className={styles.price}>${item.product.price.toFixed(2)} each</p>
            </div>

            <div className={styles.controls}>
              <div className={styles.quantitySelector}>
                <button
                  type="button"
                  aria-label="Decrease quantity"
                  className={styles.qtyBtn}
                  onClick={() => dispatch({ 
                    type: "UPDATE_QUANTITY", 
                    payload: { productId: item.product.id, quantity: item.quantity - 1 } 
                  })}
                  disabled={item.quantity <= 1}
                >
                  -
                </button>
                <span className={styles.quantityText}>{item.quantity}</span>
                <button
                  type="button"
                  aria-label="Increase quantity"
                  className={styles.qtyBtn}
                  onClick={() => dispatch({ 
                    type: "UPDATE_QUANTITY", 
                    payload: { productId: item.product.id, quantity: Math.min(item.quantity + 1, 99) } 
                  })}
                >
                  +
                </button>
              </div>

              <button
                type="button"
                className={styles.removeBtn}
                aria-label={`Remove ${item.product.name} from cart`}
                onClick={() => dispatch({ 
                  type: "REMOVE_FROM_CART", 
                  payload: { productId: item.product.id } 
                })}
              >
                Remove
              </button>
            </div>

            <div className={styles.lineTotal}>
              <strong>${(item.product.price * item.quantity).toFixed(2)}</strong>
            </div>
          </div>
        ))}
      </div>

      <footer className={styles.cartFooter}>
        <div className={styles.totalSection}>
          <h2>Total: ${cartTotal.toFixed(2)}</h2>
        </div>
        
        <CheckoutForm />
      </footer>
    </div>
  );
}