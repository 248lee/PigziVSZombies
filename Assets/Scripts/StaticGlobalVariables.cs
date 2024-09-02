using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JohnUtils
{
    public static class StaticGlobalVariables
    {
        /// <summary>
        /// This variable is used in StageWordBank. There are two methods for uniquely-random-select words.
        /// One for less stage words, and another for more stage words.
        /// <para>This variable controls what the so-called "less and more" means. </para>
        /// </summary>
        public const int UNIQUE_RANDOM_SELECT_METHOD_RATIO = 25;

        public const int REFILL_SENTENCE_THRESHOLD = 4;

        public const int REFILL_PARAGRAPH_THRESHOLD = 1;

        public const int INITIAL_NUM_OF_REFILL_SENTENCE = 8;

        public const int INITIAL_NUM_OF_REFILL_PARAGRAPH = 2;
    }
}
