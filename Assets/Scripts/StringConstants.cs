namespace GameConstants {
	public static class Strings {
		public const string GROUND_ACCEL = "Horizontal acceleration measured in units/second², used when character in grounded.";
		public const string GROUND_DECEL = "Horizontal deceleration measured in units/second², used when character is grounded.";
		public const string AIR_ACCEL = "Horizontal acceleration measured in units/second², used when character is airborne.";
		public const string AIR_DECEL = "Horizontal deceleration measured in units/second², used when character is airborne.";
		public const string WALK_SPEED = "The maximum horizontal speed the character will reach when walking, measured in units/second.";
		public const string RUN_SPEED = "The maximum horizontal speed the character will reach when running, measured in units/second.";
		public const string AIR_SPEED = "The maximum horizontal speed the character will reach when airborne, measured in units/second.";
		public const string RUN_THRESHOLD = "The threshold between walking and running. A fraction of analog input, measured from zero to one, zero indicating the stick is in neutral position, and one indicating the stick is fully tilted.\nZero => instantly run, never walk.\nOne => Mostly walk, almost never run.\nThis should have no effect when using a non-analog input, such as a keyboard.";
		public const string TERMINAL_VELOCITY = "The maximum speed to which gravity will accelerate the character, measured in units/second.";
		public const string JUMP_HEIGHT = "The maximum height in units the character can cover in a single jump.";
		public const string NUMBER_OF_JUMPS = "The maximum number of times this character can jump before landing.";
		public const string JUMP_SCALE = "A denominator to scale the height of subsequent jumps. e.g. A setting of two(2) will make double jumps half as high as first jumps. A setting of one(1) will make a double jump as high as a first jump.";
		public const string JUMP_CONTROL = "How well the character can control their jump height, measured as a fraction from zero to one.\nZero => No control. Character will always jump to JumpHeight.\nOne => Full control. Character will immediately start falling when button is released.";
		public const string COYOTE_TIME = "Time in seconds after the character falls off a ledge, during which they can still jump as if they were grounded.";
		public const string GRAVITY = "The acceleration due to gravity, in units/second² down along the y-axis. If set to a negatvie value, it will use global gravity settings, but always interpreted as a positive value.";
	}

	public static class Numbers {
		const decimal PI_M = 3.14159265358979323846264338327950m;
		const decimal TAU_M = 2 * PI_M;
		const double PI_D = (double)PI_M;
		const double TAU_D = (double)TAU_M;
		const float PI_F = (float)PI_M;
		const float TAU_F = (float)TAU_M;
	}
}
