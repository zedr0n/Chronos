using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class CreateBagHandler : ICommandHandler<CreateBagCommand>
    {
        private readonly IDomainRepository _domainRepository;

        public CreateBagHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public void Handle(CreateBagCommand command)
        {
		    var bag = new Bag(command.TargetId,command.Name);   
            _domainRepository.Save(bag);
        }
    }
    
}