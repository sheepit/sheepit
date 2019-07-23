import httpService from "./../../../common/http/http-service";

export default {
    getProjectList() {
        return httpService
            .get('api/list-projects', null);
    }
}