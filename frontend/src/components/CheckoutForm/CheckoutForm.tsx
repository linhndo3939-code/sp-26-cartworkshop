import React, { useMemo, useState } from "react";
import { useCartContext } from "../../hooks/useCartContext";
import styles from "./CheckoutForm.module.css";

interface FormData {
  name: string;
  email: string;
  zip: string;
}

const validateEmail = (value: string) =>
  /^[\w-.]+@([\w-]+\.)+[\w-]{2,4}$/.test(value);

const validateZip = (value: string) => /^\d{5}$/.test(value);

export function CheckoutForm() {
  const { dispatch, cartTotal } = useCartContext();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);
  
  const [formData, setFormData] = useState<FormData>({
    name: "",
    email: "",
    zip: "",
  });

  const [touched, setTouched] = useState<Set<keyof FormData>>(new Set());

  const errors = useMemo(() => {
    return {
      name: !formData.name.trim() ? "Name is required" : "",
      email: !formData.email
        ? "Email is required"
        : validateEmail(formData.email)
        ? ""
        : "Enter a valid email",
      zip: !formData.zip
        ? "Zip code is required"
        : validateZip(formData.zip)
        ? ""
        : "Zip code must be 5 digits",
    };
  }, [formData]);

  const isFormValid = useMemo(
    () => !errors.name && !errors.email && !errors.zip,
    [errors]
  );

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleBlur = (e: React.FocusEvent<HTMLInputElement>) => {
    setTouched((prev) => new Set(prev).add(e.target.name as keyof FormData));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    setTouched(new Set(Object.keys(formData) as Array<keyof FormData>));

    if (!isFormValid) return;

    setIsSubmitting(true);

    // Simulate API Call
    setTimeout(() => {
      dispatch({ type: "CLEAR_CART" });
      setIsSuccess(true);
      setIsSubmitting(false);
    }, 1500);
  };

  if (isSuccess) {
    return (
      <div className={styles.success} role="alert">
        <h3>O-H! Order Placed Successfully!</h3>
        <p>Your Buckeye gear is on the way.</p>
      </div>
    );
  }

  return (
    <form onSubmit={handleSubmit} className={styles.form}>
      <h2 className={styles.heading}>Shipping Information</h2>
      
      <div className={styles.field}>
        <label htmlFor="name">Name</label>
        <input
          id="name"
          name="name"
          type="text"
          value={formData.name}
          onChange={handleChange}
          onBlur={handleBlur}
          aria-invalid={touched.has("name") && Boolean(errors.name)}
          aria-describedby="name-error"
        />
        {touched.has("name") && errors.name ? (
          <div id="name-error" className={styles.error} role="alert">
            {errors.name}
          </div>
        ) : null}
      </div>

      <div className={styles.field}>
        <label htmlFor="email">Email</label>
        <input
          id="email"
          name="email"
          type="email"
          value={formData.email}
          onChange={handleChange}
          onBlur={handleBlur}
          aria-invalid={touched.has("email") && Boolean(errors.email)}
          aria-describedby="email-error"
        />
        {touched.has("email") && errors.email ? (
          <div id="email-error" className={styles.error} role="alert">
            {errors.email}
          </div>
        ) : null}
      </div>

      <div className={styles.field}>
        <label htmlFor="zip">Zip Code</label>
        <input
          id="zip"
          name="zip"
          type="text"
          value={formData.zip}
          onChange={handleChange}
          onBlur={handleBlur}
          aria-invalid={touched.has("zip") && Boolean(errors.zip)}
          aria-describedby="zip-error"
        />
        {touched.has("zip") && errors.zip ? (
          <div id="zip-error" className={styles.error} role="alert">
            {errors.zip}
          </div>
        ) : null}
      </div>

      <button 
        type="submit" 
        className={styles.submitBtn}
        disabled={isSubmitting}
      >
        {isSubmitting ? "Processing..." : `Pay $${cartTotal.toFixed(2)}`}
      </button>
    </form>
  );
}