using Cardinal_System_Common;

namespace Cardinal_System_HeathCliff.InternalMessage
{
    public class ComponentIdRequest
    {
        public CsComponentConnection CsComponentConnection { get; private set; }

        public ComponentIdRequest(CsComponentConnection csComponentConnection)
        {
            CsComponentConnection = csComponentConnection;
        }
    }
}
