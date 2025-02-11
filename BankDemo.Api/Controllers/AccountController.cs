using Microsoft.AspNetCore.Mvc;
using MediatR;
using BankDemo.Application.Commands;
using BankDemo.Domain.Account;
using BankDemo.Application.Queries;

[ApiController]
[Route("api/account")]
[Tags("Account Management")]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IMediator mediator, ILogger<AccountController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region Account Management

    /// <summary>
    /// Creates a new bank account
    /// </summary>
    /// <param name="command">Account creation details</param>
    /// <returns>The created account</returns>
    [HttpPost]
    public async Task<ActionResult<Account>> Create(CreateAccountCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all bank accounts
    /// </summary>
    /// <returns>List of all accounts</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Account>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllAccountsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific bank account by ID
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <returns>The requested account</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Account>> GetById(Guid id)
    {
        try
        {
            var query = new GetAccountByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        catch (AccountNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Updates an existing bank account
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="command">Account update details</param>
    /// <returns>The updated account</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<Account>> Update(Guid id, [FromBody] UpdateAccountCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (AccountNotFoundException)
        {
            return NotFound();
        }
        catch (AccountConcurrencyException ex)
        {
            return Conflict(new 
            { 
                message = ex.Message,
                currentVersion = ex.ActualVersion,
                providedVersion = ex.ExpectedVersion
            });
        }
    }

    /// <summary>
    /// Deletes a bank account
    /// </summary>
    /// <param name="id">Account ID to delete</param>
    /// <returns>The deleted account</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<Account>> Delete(Guid id)
    {
        try
        {
            var command = new DeleteAccountCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (AccountNotFoundException)
        {
            return NotFound();
        }
    }

    #endregion

    #region Transactions

    /// <summary>
    /// Deposits money into a bank account
    /// </summary>
    /// <param name="command">Deposit details</param>
    /// <returns>The updated account</returns>
    [HttpPost("transactions/deposit")]
    [Tags("Transactions")]
    public async Task<ActionResult<Account>> Deposit([FromBody] DepositCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (AccountNotFoundException)
        {
            return NotFound();
        }
        catch (AccountConcurrencyException ex)
        {
            return Conflict(new 
            { 
                message = ex.Message,
                currentVersion = ex.ActualVersion,
                providedVersion = ex.ExpectedVersion
            });
        }
    }

    /// <summary>
    /// Transfers money between bank accounts
    /// </summary>
    /// <param name="command">Transfer details</param>
    /// <returns>The updated source account</returns>
    [HttpPost("transactions/transfer")]
    [Tags("Transactions")]
    public async Task<ActionResult<Account>> Transfer([FromBody] TransferCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (AccountNotFoundException)
        {
            return NotFound();
        }
        catch (AccountConcurrencyException ex)
        {
            return Conflict(new 
            { 
                message = ex.Message,
                currentVersion = ex.ActualVersion,
                providedVersion = ex.ExpectedVersion
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    #endregion
}
