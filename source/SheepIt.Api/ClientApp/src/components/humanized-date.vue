<template>
    <span :title="formattedDate">
        {{ humanizedDate }}
    </span>
</template>

<script>
import moment from 'moment';

export default {
    name: "HumanizedDate",

    props: ['date'],

    computed: {
        humanizedDate() {
            return moment.duration(moment.utc(this.date).diff(moment())).humanize(true);
        },
        formattedDate() {
            return moment.utc(this.date).local().format('YYYY-MM-DD hh:mm A');
        }
    }
}
</script>