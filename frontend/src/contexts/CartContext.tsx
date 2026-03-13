import React, { createContext, useReducer, ReactNode, useMemo } from "react";
import { CartItem, CartAction } from "../../types/cart";
import { cartReducer, initialCartState } from "../../reducers/cartReducer";

// 1. Define everything the context provides
interface CartContextType {
  items: CartItem[];
  isOpen: boolean;         // Added
  cartItemCount: number;   // Added
  cartTotal: number;
  dispatch: React.Dispatch<CartAction>;
}

// 2. Export the Context
export const CartContext = createContext<CartContextType | null>(null);

// 3. The Provider
export const CartProvider = ({ children }: { children: ReactNode }) => {
  const [state, dispatch] = useReducer(cartReducer, initialCartState);

  // Derived values (Part 3 of Lab PDF)
  const cartItemCount = useMemo(
    () => state.items.reduce((sum, item) => sum + item.quantity, 0),
    [state.items]
  );

  const cartTotal = useMemo(
    () => state.items.reduce((sum, item) => sum + (item.product.price * item.quantity), 0),
    [state.items]
  );

  // Use useMemo for performance to prevent unnecessary re-renders
  const value = useMemo(() => ({
    items: state.items,
    isOpen: state.isOpen,
    cartItemCount,
    cartTotal,
    dispatch,
  }), [state.items, state.isOpen, cartItemCount, cartTotal]);

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
};