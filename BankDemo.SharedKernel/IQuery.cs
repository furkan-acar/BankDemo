using MediatR;

namespace BankDemo.SharedKernel;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
