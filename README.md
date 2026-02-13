# 🤖 AI Chatbot z kolejką zadań
Aplikacja chat bot z React UI, która wysyła pytania do .NET 8 API. Prompty zapisują się w PostgreSQL z real-time statusami (Pending → Processing → Completed), a przetwarzanie odbywa się asynchronicznie przez RabbitMQ (MassTransit) + OpenAI.

## 🏗️ Stack
React 18 (UI chat) → .NET 8 API → RabbitMQ (MassTransit) → OpenAI GPT-4o-mini
