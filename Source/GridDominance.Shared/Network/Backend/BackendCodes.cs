﻿namespace GridDominance.Shared.Network.Backend
{
	public static class BackendCodes
	{
		/* ======== 99 INTERNAL ========= */
		public const int INTERNAL_EXCEPTION      = 99099;
		public const int MISSING_PARAMETER       = 99001;
		public const int PARAMETER_HASH_MISMATCH = 99002;
		public const int INVALID_PARAMETER       = 99003;
		public const int USER_BY_ID_NOT_FOUND    = 99004;
		public const int WRONG_PASSWORD          = 99005;
		public const int USER_BY_NAME_NOT_FOUND  = 99006;

		/* ======== 11 UPGRADE-USER ========= */
		public const int UPGRADE_USER_DUPLICATE_USERNAME  = 10001;
		public const int UPGRADE_USER_ACCOUNT_ALREADY_SET = 11002;

		/* ======== 12 SET-SCORE ========= */
		public const int SET_SCORE_INVALID_TIME  = 12001;
		public const int SET_SCORE_INVALID_SCORE = 12002;
		public const int SET_SCORE_INVALID_LVLID = 12003;
		public const int SET_SCORE_INVALID_DIFF  = 12004;

		/* ======== 13 SET-SCORE ========= */
		public const int CRON_INTERNAL_ERR  = 13001;

		/* ======== 14 MERGE-LOGIN ========= */
		public const int MERGE_INVALID_TIME  = 14001;
		public const int MERGE_INVALID_LVLID = 14002;
		public const int MERGE_INVALID_DIFF  = 14003;
	}
}
