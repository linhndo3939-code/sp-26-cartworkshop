# Buckeye Marketplace - AI Agent Instructions

## Project Overview
Buckeye Marketplace is a full-stack e-commerce application built with:
- **Frontend:** React + TypeScript (Vite)
- **Backend:** .NET Web API with Entity Framework Core
- **Database:** In-Memory for development

## Architecture
- Frontend runs on http://localhost:5173
- Backend runs on http://localhost:5228
- API base path: /api/

## Frontend Conventions
- Components live in `src/components/`
- Style: CSS Modules (`*.module.css`)
- State management: useReducer + Context API for the cart
- Functional components only with TypeScript strict mode

## Rules for AI
- Always include TypeScript types.
- Always include aria-labels on interactive elements.
- Use named exports for components.
