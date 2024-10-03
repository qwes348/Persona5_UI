// To activate the extension sample, uncomment the next line:
//#define ENABLE_EXTENSION_SAMPLE

#if ENABLE_EXTENSION_SAMPLE && UNITY_EDITOR
namespace Nementic.SelectionUtility.Samples
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Demonstrates how to add a custom tab to the filter toolbar.
	/// </summary>
	public static class CustomExtension
	{
		[InitializeOnLoadMethod]
		private static void RegisterCustomFilter()
		{
			// To test the example methods, copy paste their
			// assignment to the modifier delegate here or simply read on.
			SelectionPopupExtensions.FilterModifier = AddFilter;
		}

		private static IEnumerable<DataFilter> AddFilter(List<DataFilter> filters)
		{
			filters.Add(new DataFilter("Tag", HasPlayerTag));
			return filters;
		}

		private static IEnumerable<DataFilter> SkipAndAdd(List<DataFilter> filters)
		{
			// It is also possible to return filters via the enumerator.
			foreach (var filter in filters)
			{
				// Skip one of the default filters,
				// but keep all others.
				if (filter.ShortName != "UI")
					yield return filter;
			}
			yield return new DataFilter("Tag", HasPlayerTag);
		}

		private static IEnumerable<DataFilter> ReorderFilters(List<DataFilter> filters)
		{
			// Changing the filters list or returning a new collection is possible.
			return filters.OrderBy(x => x.ShortName);
		}

		private static IEnumerable<DataFilter> ReplaceFilter(List<DataFilter> filters)
		{
			// Easy way to replace the third filter with a custom one.
			// A more robust method would be to find it by name.
			filters[2] = new DataFilter("Bg", go =>
			{
				var renderer = go.GetComponent<SpriteRenderer>();
				return renderer != null && renderer.sortingLayerName == "Background";
			});
			return filters;
		}

		private static bool HasPlayerTag(GameObject go)
		{
			// If the filter is active, only show items that are tagged.
			return go.CompareTag("Player");
		}
	}
}
#endif
