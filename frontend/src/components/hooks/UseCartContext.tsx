import { useContext } from "react";
import { CartContext } from "../../contexts/CartContext";

export const useCartContext = () => {
  const context = useContext(CartContext);
  
  if (context === undefined || context === null) {
    throw new Error("useCartContext must be used within a CartProvider");
  }
  
  return context; 
};