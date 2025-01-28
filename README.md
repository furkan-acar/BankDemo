```mermaid
graph LR
    BankDemo.Api --> BankDemo.Application
    BankDemo.Api --> BankDemo.Infrastructure
    BankDemo.Api --> BankDemo.Domain
    BankDemo.Api --> BankDemo.SharedKernel
    BankDemo.Application --> BankDemo.Domain
    BankDemo.Application --> BankDemo.SharedKernel
    BankDemo.Infrastructure --> BankDemo.Domain
    BankDemo.Infrastructure --> BankDemo.SharedKernel
    BankDemo.Domain --> BankDemo.SharedKernel