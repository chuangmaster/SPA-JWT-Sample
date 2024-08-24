<template>
    <div>
        <ul>
            <li v-for="forecast in forecasts" :key="forecast.date">
                {{ forecast.date }} - {{ forecast.temperatureC }}°C - {{ forecast.summary }}
            </li>
        </ul>
    </div>
</template>

<script>
import axios from 'axios';

export default {
    data() {
        return {
            forecasts: [],
        };
    },
    mounted() {
        this.getWeatherForecast();
    },
    methods: {
        async getWeatherForecast() {
            try {

                //axios.defaults.withCredentials = true; // 確保請求包含 cookies
                const response = await axios.get('https://localhost:7173/weatherforecast', {
                    withCredentials: true,
                }).then(response => {
                    this.forecasts = response.data;
                })
                    .catch(error => {
                        console.error('Error fetching weather forecast:', error);
                    });

            } catch (error) {
                console.log(error);
                console.error('Error fetching weather forecast:', error);
                alert('Failed to fetch weather forecast');
            }
        },
    },
};
</script>