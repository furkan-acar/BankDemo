using MediatR;

namespace BankDemo.SharedKernel;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}