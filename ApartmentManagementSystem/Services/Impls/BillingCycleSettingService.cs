using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Interfaces;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Services.Interfaces;

namespace ApartmentManagementSystem.Services.Impls
{
    internal class BillingCycleSettingService : IBillingCycleSettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBillingCycleSettingRepository _billingCycleSettingRepository;
        public BillingCycleSettingService(IUnitOfWork unitOfWork, IBillingCycleSettingRepository billingCycleSettingRepository)
        {
            _unitOfWork = unitOfWork;
            _billingCycleSettingRepository = billingCycleSettingRepository;
        }

        public async Task CreateBillingCycleSetting(BillingCycleSettingDto request)
        {
            if (_billingCycleSettingRepository.List(b => b.ApartmentBuildingId.Equals(request.ApartmentBuildingId)).Any())
                throw new DomainException(ErrorCodeConsts.BillingCycleIsSet, ErrorCodeConsts.BillingCycleIsSet, System.Net.HttpStatusCode.BadRequest);

            var billingCycleSetting = new BillingCycleSetting()
            {
                ApartmentBuildingId = request.ApartmentBuildingId,
                ClosingDayOfMonth = request.ClosingDayOfMonth,
                PaymentDueDate = request.PaymentDueDate
            };
            await _billingCycleSettingRepository.Add(billingCycleSetting);
            await _unitOfWork.CommitAsync();
        }

        public async Task<BillingCycleSettingDto> GetBillingCycleSetting(string apartmentBuildingId)
        {
            var billingCycleSetting = _billingCycleSettingRepository.List(b => b.ApartmentBuildingId.Equals(new Guid(apartmentBuildingId))).FirstOrDefault();
            if (billingCycleSetting == null) return null;

            return new BillingCycleSettingDto()
            {
                ApartmentBuildingId = billingCycleSetting.ApartmentBuildingId,
                ClosingDayOfMonth = billingCycleSetting.ClosingDayOfMonth,
                PaymentDueDate = billingCycleSetting.PaymentDueDate
            };
        }
    }
}