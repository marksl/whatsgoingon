namespace Model
{
    public partial class BuildDiff
    {
        public string Category
        {
            get
            {
                if (CurrentBuildDataId.HasValue)
                    return CurrentBuildData.Category;

                if (PreviousBuildDataId.HasValue)
                    return PreviousBuildData.Category;

                return "Unknown";
            }
        }

        public string Type
        {
            get
            {
                if (CurrentBuildDataId.HasValue)
                    return CurrentBuildData.Type;

                if (PreviousBuildDataId.HasValue)
                    return PreviousBuildData.Type;

                return "Unknown";
            }
        }

        public string PreviousData
        {
            get
            {
                if (PreviousBuildDataId.HasValue)
                    return PreviousBuildData.Data;

                return string.Empty;
            }
        }

        public string CurrentData
        {
            get
            {
                if (CurrentBuildDataId.HasValue)
                    return CurrentBuildData.Data;

                return string.Empty;
            }
        }
    }
}