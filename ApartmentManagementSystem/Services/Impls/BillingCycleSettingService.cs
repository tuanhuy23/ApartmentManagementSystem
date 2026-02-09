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
            if (request.Id != Guid.Empty)
            {
                var existingSetting = _billingCycleSettingRepository.List(b => b.Id == request.Id).FirstOrDefault();
                if (existingSetting == null)
                    throw new DomainException(ErrorCodeConsts.BillingCycleSettingIsNotFound, ErrorCodeConsts.BillingCycleSettingIsNotFound, System.Net.HttpStatusCode.NotFound);

                existingSetting.ClosingDayOfMonth = request.ClosingDayOfMonth;
                existingSetting.PaymentDueDate = request.PaymentDueDate;

                _billingCycleSettingRepository.Update(existingSetting);
                await _unitOfWork.CommitAsync();
                return;
            }
            if (_billingCycleSettingRepository.List(b => b.ApartmentBuildingId.Equals(request.ApartmentBuildingId)).Any() && request.Id == Guid.Empty)
                throw new DomainException(ErrorCodeConsts.BillingCycleAlreadySet, ErrorCodeConsts.BillingCycleAlreadySet, System.Net.HttpStatusCode.BadRequest);
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
                Id = billingCycleSetting.Id,
                ApartmentBuildingId = billingCycleSetting.ApartmentBuildingId,
                ClosingDayOfMonth = billingCycleSetting.ClosingDayOfMonth,
                PaymentDueDate = billingCycleSetting.PaymentDueDate
            };
        }
    }
}