import type { ProductResponse } from "./product";

export type CartItem = {
  product: ProductResponse;
  quantity: number;
  /** Derived from product.price so cart totals can be computed without assuming product object shape */
  price: number;
};

export type CartState = {
  items: CartItem[];
  isOpen: boolean;
};

export type CartAction =
  | { type: "ADD_TO_CART"; payload: { product: ProductResponse; quantity?: number } }
  | { type: "REMOVE_FROM_CART"; payload: { productId: number } }
  | { type: "UPDATE_QUANTITY"; payload: { productId: number; quantity: number } }
  | { type: "CLEAR_CART" }
  | { type: "TOGGLE_CART" };
