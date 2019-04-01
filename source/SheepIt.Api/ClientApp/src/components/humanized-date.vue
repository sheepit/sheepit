<template>
    <tooltip
        data-placement="top"
        :text="formattedDate"
    >
        <mark>{{ humanizedDate }}</mark>
    </tooltip>
</template>

<script>
import moment from 'moment';

export default {
    name: "HumanizedDate",

    props: ['date'],

    computed: {
        humanizedDate() {
            return moment.duration(moment(this.date).diff(moment())).humanize(true);
        },
        formattedDate() {
            return moment(this.date).format('YYYY-MM-DD hh:mm')
        }
    }
}
</script>