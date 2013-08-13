namespace Model
{
    public partial class Build
    {

        public string Version
        {
            get
            {
                string build = 0.ToString("D8");
                if (VBuild.HasValue)
                    build = VBuild.Value.ToString("D8");

                return string.Format("{0}.{1}.{2}.{3}",
                                     VMajor.ToString("D6"), VMinor.ToString("D6"), VPatch.ToString("D6"), build);
            }
        }

        protected bool Equals(Build other)
        {
            return VPatch == other.VPatch && VMinor == other.VMinor && VMajor == other.VMajor && VBuild == other.VBuild && string.Equals(GitSha, other.GitSha);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals((Build) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = VPatch;
                hashCode = (hashCode*397) ^ VMinor;
                hashCode = (hashCode*397) ^ VMajor;
                hashCode = (hashCode*397) ^ VBuild.GetHashCode();
                hashCode = (hashCode*397) ^ (GitSha != null ? GitSha.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
