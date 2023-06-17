<script lang="ts">
	import RangeSlider from "svelte-range-slider-pips";
	let sliderValue = 0; // the value from the slider
	let realValue = 0; // the mapped value

	const mapSliderValue = (value: number) => {
		const scale = [1, 2, 3, 4, 5, 10, 15, 25, 50, 100];
		return scale[value];
	}

	$: realValue = mapSliderValue(sliderValue);
</script>

<RangeSlider id=small bind:value={sliderValue} min={0} max={9} step={1} />

<p>
	The real value is: {realValue}
</p>

<!-- your styles here -->
