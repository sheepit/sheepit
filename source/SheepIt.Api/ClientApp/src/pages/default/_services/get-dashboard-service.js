import httpService from "../../../common/http/http-service";

export default {
    getLastDeploymentsList() {
        return httpService
            .get('api/dashboard/get-dashboard', null);
    }
}
