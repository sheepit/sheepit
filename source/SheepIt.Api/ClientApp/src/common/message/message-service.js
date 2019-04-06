import toastr from 'toastr';
import 'toastr/build/toastr.css';

export default {
    getOptions() {
        return {
            toastClass: 'alert',
            iconClasses: {
                error: 'alert-error',
                info: 'alert-info',
                success: 'alert-success',
                warning: 'alert-warning'
            }
        };
    },

    error(message) {
        toastr.error(message, null, this.getOptions());
    },
    
    success(message) {
        toastr.success(message, null, this.getOptions());
    },
    
    warning(message) {
        toastr.warning(message, null, this.getOptions());
    },
    
    info(message) {
        toastr.info(message, null, this.getOptions());
    }
};