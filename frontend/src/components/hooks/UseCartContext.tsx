import { useContext } from "react";
import { CartContext } from "../contexts/CartContext"; 

export const useCartContext = () => {
  const context = useContext(CartContext);
  
  // This check is required by Part 3 of your Lab PDF
  if (context === undefined || context === null) {
    throw new Error("useCartContext must be used within a CartProvider");
  }
  
  return context; 
};