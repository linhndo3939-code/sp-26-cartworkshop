import { Link } from "react-router-dom";
import type { ProductResponse } from "../../types/product"; 
import { AddToCartButton } from "../AddToCartButton/AddToCartButton"; 
import styles from "./ProductCard.module.css";

interface ProductCardProps {
  product: ProductResponse;
}

export default function ProductCard({ product }: ProductCardProps) {
  return (
    <div className={styles.cardContainer}>
      <Link to={`/products/${product.id}`} className={styles.card}>
        <div className={styles.imageWrapper}>
          {product.imageUrl ? (
            <img
              src={product.imageUrl}
              alt={product.name ?? "Product"}
              className={styles.image}
            />
          ) : (
            <div className={styles.placeholder}>
              <span>📦</span>
            </div>
          )}
        </div>
        <div className={styles.body}>
          <span className={styles.category}>{product.categoryName}</span>
          <h3 className={styles.name}>{product.name}</h3>
          <p className={styles.price}>${product.price.toFixed(2)}</p>
        </div>
      </Link>

      <div className={styles.buttonWrapper}>
        {/* Pass the whole object to avoid 'missing properties' errors */}
        <AddToCartButton product={product} />
      </div>
    </div>
  );
}