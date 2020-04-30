import httpService from "../../../common/http/http-service";

export default {
    getLastDeploymentsList() {
        return httpService
            .get('frontendApi/dashboard/get-dashboard', null);
    }
}
