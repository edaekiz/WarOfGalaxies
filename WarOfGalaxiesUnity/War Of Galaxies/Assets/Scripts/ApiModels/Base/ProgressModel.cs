using System;

namespace Assets.Scripts.ApiModels.Base
{
    [Serializable]
    public abstract class ProgressModel
    {
        public DateTime BeginDate;
        public DateTime EndDate;

        public void CalculateDates(double passedTime, double leftTime)
        {
            BeginDate = DateTime.UtcNow.AddSeconds(-passedTime);
            EndDate = BeginDate.AddSeconds(leftTime);
        }
    }
}
