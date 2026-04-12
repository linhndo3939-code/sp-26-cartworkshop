// @ts-nocheck
/* eslint-disable */
// @ts-nocheck
/* eslint-disable */
import { test, expect } from '@playwright/test';

test.describe('Shopping Cart Flow', () => {
  test('should allow a user to log in and add an item to the cart', async ({ page }) => {
    // ... rest of the code
    // 1. Navigate to the login page
    await page.goto('/login');

    // 2. Perform login
    await page.fill('input[name="username"]', 'testuser');
    await page.fill('input[name="password"]', 'password123');
    await page.click('button[type="submit"]');

    // 3. Navigate to Products and add the first item
    await page.goto('/products');
    // Wait for the "Add to Cart" button to appear and click the first one
    const addToCartButton = page.locator('button:has-text("Add to Cart")').first();
    await expect(addToCartButton).toBeVisible();
    await addToCartButton.click();

    // 4. Navigate to Cart and verify the item is there
    await page.goto('/cart');
    
    // Check for a success message or the presence of the product name
    const cartItem = page.locator('.cart-item-list');
    await expect(cartItem).toBeVisible();
    
    // Verify the cart isn't empty
    const emptyMessage = page.locator('text=Your cart is empty');
    await expect(emptyMessage).not.toBeVisible();
  });
});