🤖 AI Prompt Processor – aplikacja do wysyłania promptów do OpenAI i przetwarzania odpowiedzi.

## Opis projektu

Projekt zawiera:

* **Backend (.NET 8)** – API do obsługi promptów, integracja z OpenAI.
* **Frontend (React + Nginx)** – interfejs do wysyłania promptów i wyświetlania odpowiedzi.
* **Docker Compose** – przygotowane pliki Dockerfile i `docker-compose.yml`, aby uruchomić całość jednym poleceniem.

> ⚠️ **Uwaga:** Podczas testów Docker Compose wystąpił problem z połączeniem backendu z bazą SQL Server (`PromptDb`).
> Docker Compose `up` może zakończyć się błędem po stronie backendu przy pierwszym uruchomieniu.

## Manualne uruchomienie (działa poprawnie)

Jeśli Docker sprawia problemy, możesz uruchomić frontend i backend ręcznie:

1. **Frontend:**

   * Otwórz folder `frontend` w terminalu i wpisz:

     ```bash
     npm install
     npm start
     ```
   * Frontend dostępny pod: [http://localhost:3000](http://localhost:3000)

2. **Backend:**

   * Otwórz projekt `PromptApi` w Visual Studio i uruchom jako **HTTPS** (`Run as HTTPS`).

3. **Konfiguracja OpenAI:**

   * Stwórz plik `.env` w katalogu głównym projektu.
   * Podaj w nim swój klucz do OpenAI (ten klucz podam w emailu):

     ```env
     OPENAI_API_KEY=twój_klucz_openai
     OPENAI_MODEL=gpt-4o-mini
     

## Testowanie AI

1. Wejdź na [http://localhost:3000/](http://localhost:3000/)
2. Wpisz dowolny prompt i kliknij **Wyślij**.
3. Zobaczysz trzy statusy: **Pending**, **Processing**, **Completed**.
4. Aby przetestować flagę **Failed**, wpisz prompt:

   
   error test

   – backend zasymuluje nieudaną odpowiedź AI.

## Uwagi końcowe

* Projekt powstał jako zadanie rekrutacyjne.
* Docker Compose może wymagać dodatkowego czasu lub manualnej konfiguracji SQL Server.
* Manualne uruchomienie działa i pozwala przetestować aplikację od razu.
