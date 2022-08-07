namespace YourVRExperience.VR
{
	public class OculusEventData
	{
		private string m_nameEvent;
		private float m_time;
		private object[] m_listParameters;

		public string NameEvent
		{
			get { return m_nameEvent; }
		}
		public float Time
		{
			get { return m_time; }
			set { m_time = value; }
		}
		public object[] ListParameters
		{
			get { return m_listParameters; }
		}

		public OculusEventData(string _nameEvent, float _time, params object[] _list)
		{
			m_nameEvent = _nameEvent;
			m_time = _time;
			m_listParameters = _list;
		}

		public void Destroy()
		{
			m_listParameters = null;
		}

		public string ToStringParameters()
		{
			string parameters = "";
			if (m_listParameters != null)
			{
				for (int i = 0; i < m_listParameters.Length; i++)
				{
					if (m_listParameters[i] is string)
					{
						parameters += (string)m_listParameters[i];
					}
					else
					{
						parameters += m_listParameters[i].ToString();
					}
					if (i + 1 < m_listParameters.Length)
					{
						parameters += ",";
					}
				}
			}
			return parameters;
		}

		public override string ToString()
		{
			string parameters = ToStringParameters();
			if (parameters.Length > 0)
			{
				parameters = "," + parameters;
			}

			return m_nameEvent + parameters;
		}
	}
}