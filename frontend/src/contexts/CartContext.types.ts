import { createContext, type Dispatch } from "react"; // Added 'type' here
import type { CartAction, CartItem } from "../types/cart";

export interface CartContextValue {
  items: CartItem[];
  isOpen: boolean;
  dispatch: Dispatch<CartAction>;
  cartItemCount: number;
  cartTotal: number;
}

export const CartContext = createContext<CartContextValue | null>(null);