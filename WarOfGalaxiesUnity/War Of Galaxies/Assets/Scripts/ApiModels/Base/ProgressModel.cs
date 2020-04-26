using System;

namespace Assets.Scripts.ApiModels.Base
{
    [Serializable]
    public abstract class ProgressModel
    {
        public DateTime BeginDate;
        public DateTime EndDate;

        public void CalculateDates(double leftTime)
        {
            BeginDate = DateTime.UtcNow;
            EndDate = BeginDate.AddSeconds(leftTime);
        }
    }
}
