namespace ScalarDemo.Helper;


public static class Contants
{
	public static class ApiVersions
	{
		public static readonly ApiVersion V1_0 = new(1, 0);
		public static readonly ApiVersion V2_0 = new(2, 0);
		public static readonly ApiVersion V2_2 = new(2, 2);
		public static readonly ApiVersion V3_0 = new(3, 0);

		public static readonly ApiVersion[] All = [V1_0, V2_0, V2_2, V3_0];
	}
}
