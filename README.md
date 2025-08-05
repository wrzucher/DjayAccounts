# DjayAccounts

Test Project for Interview.
API to be used to manage accounts within a bank.

## Requirements

- Support for different account types with the possibility for future expansion. For now, Current and Savings accounts are the only requirements.
- Ability to create and retrieve accounts.
- Ability to freeze(disable) an account.
- Written in C# (.NET 8 and onwards).

## Notes

- This is a stateless service: it does not keep any session data in memory.
- The project structure is simple, as we do not expect a high load and do not have such a requirement.
- Idempotency is handled with external AccountId and ClientId.
- It will be easy to scale later if needed, since it is stateless and follows standard REST design principles.
- Actually, I forgot that AutoMapper changed its license, so it is better to use another library.
- Authorization was not added because I did not see it in the requirements. It’s not a big deal to add it — just ask me.