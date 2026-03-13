import type { CartAction, CartItem, CartState } from "../types/cart";

export const initialCartState: CartState = {
  items: [],
  isOpen: false,
};

export function cartReducer(state: CartState, action: CartAction): CartState {
  switch (action.type) {
    case "ADD_TO_CART": {
      const { product, quantity = 1 } = action.payload;
      const existingItem = state.items.find((item: CartItem) => item.product.id === product.id);

      if (existingItem) {
        return {
          ...state,
          items: state.items.map((item: CartItem) =>
         item.product.id === product.id
          ? { ...item, quantity: item.quantity + quantity }
          : item
),
        };
      }

      return {
        ...state,
        items: [...state.items, { product, quantity, price: product.price }],
      };
    }

    case "REMOVE_FROM_CART":
      return {
        ...state,
        items: state.items.filter((item: CartItem) => item.product.id !== action.payload.productId),
      };

    case "UPDATE_QUANTITY": {
      const { productId, quantity } = action.payload;

      // Handle removal if quantity drops to 0 or less (Lab Requirement Page 5)
      if (quantity <= 0) {
        return {
          ...state,
          items: state.items.filter((item: CartItem) => item.product.id !== productId),
        };
      }

      return {
        ...state,
        items: state.items.map((item: CartItem) =>
          item.product.id === productId 
            ? { ...item, quantity: Math.min(quantity, 99) } // Clamp max at 99 (Lab Page 10)
            : item
        ),
      };
    }

    case "CLEAR_CART":
      return {
        ...state,
        items: [],
      };

    case "TOGGLE_CART":
      return {
        ...state,
        isOpen: !state.isOpen,
      };

    // Exhaustive checking: If a new action is added to the type but not handled here,
    // TypeScript will throw an error on 'action'.
    default: {
  const _exhaustiveCheck: never = action;
  console.warn(`Unhandled action type: ${_exhaustiveCheck}`); // This "uses" the variable
  return state;
    }
  }
}