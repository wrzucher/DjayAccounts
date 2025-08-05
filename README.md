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
- Project structure is simple, as we do not expect high load and we do not have a such requirement.
- Idempotency is handled with external AccountId and ClientId.
- Easy to scale later if needed, since it is stateless and uses standard REST design.