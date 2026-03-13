import React, { useEffect, useState } from "react";
// This now works because we added the hook to CartContext.tsx
import { useCartContext } from "../../contexts/CartContext"; 
// FIXED: Changed 'product' to 'products' to match your plural filename
import type { ProductResponse } from "../../types/product"; 
import styles from "./AddToCartButton.module.css";

interface AddToCartButtonProps {
  product: ProductResponse;
}

export function AddToCartButton({ product }: AddToCartButtonProps) {
  const { dispatch } = useCartContext();
  const [added, setAdded] = useState(false);

  useEffect(() => {
    if (!added) return;
    const timer = window.setTimeout(() => setAdded(false), 1500);
    return () => window.clearTimeout(timer);
  }, [added]);

  const handleClick = () => {
    // Matches your CartAction: payload: { product }
    dispatch({ type: "ADD_TO_CART", payload: { product } });
    setAdded(true);
  };

  return (
    <button
      type="button"
      className={`${styles.button} ${added ? styles.added : ""}`}
      onClick={handleClick}
      disabled={added}
      aria-label={`Add ${product.name} to cart`}
    >
      <div className={styles.label}>
        {added ? "Added!" : "Add to Cart"}
      </div>
    </button>
  );
}