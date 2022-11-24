/* tslint:disable */
/* eslint-disable */
/**
 * Admin.NET
 * 让 .NET 开发更简单、更通用、更流行。前后端分离架构(.NET6/Vue3)，开箱即用紧随前沿技术。<br/><a href='https://gitee.com/zuohuaijun/Admin.NET/'>https://gitee.com/zuohuaijun/Admin.NET</a>
 *
 * OpenAPI spec version: 1.0.0
 * Contact: 515096995@qq.com
 *
 * NOTE: This class is auto generated by the swagger code generator program.
 * https://github.com/swagger-api/swagger-codegen.git
 * Do not edit the class manually.
 */
import globalAxios, { AxiosResponse, AxiosInstance, AxiosRequestConfig } from 'axios';
import { Configuration } from '../configuration';
// Some imports not used depending on template conditions
// @ts-ignore
import { BASE_PATH, COLLECTION_FORMATS, RequestArgs, BaseAPI, RequiredError } from '../base';
import { AdminResultListSchedulerModel } from '../models';
import { AdminResultScheduleResult } from '../models';
import { JobInput } from '../models';
/**
 * SysJobApi - axios parameter creator
 * @export
 */
export const SysJobApiAxiosParamCreator = function (configuration?: Configuration) {
    return {
        /**
         * 
         * @summary 增加作业任务
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        sysJobAddPost: async (options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            const localVarPath = `/sysJob/add`;
            // use dummy base URL string because the URL constructor only accepts absolute URLs.
            const localVarUrlObj = new URL(localVarPath, 'https://example.com');
            let baseOptions;
            if (configuration) {
                baseOptions = configuration.baseOptions;
            }
            const localVarRequestOptions :AxiosRequestConfig = { method: 'POST', ...baseOptions, ...options};
            const localVarHeaderParameter = {} as any;
            const localVarQueryParameter = {} as any;

            // authentication Bearer required

            const query = new URLSearchParams(localVarUrlObj.search);
            for (const key in localVarQueryParameter) {
                query.set(key, localVarQueryParameter[key]);
            }
            for (const key in options.params) {
                query.set(key, options.params[key]);
            }
            localVarUrlObj.search = (new URLSearchParams(query)).toString();
            let headersFromBaseOptions = baseOptions && baseOptions.headers ? baseOptions.headers : {};
            localVarRequestOptions.headers = {...localVarHeaderParameter, ...headersFromBaseOptions, ...options.headers};

            return {
                url: localVarUrlObj.pathname + localVarUrlObj.search + localVarUrlObj.hash,
                options: localVarRequestOptions,
            };
        },
        /**
         * 
         * @summary 删除作业任务
         * @param {JobInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        sysJobDeletePost: async (body?: JobInput, options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            const localVarPath = `/sysJob/delete`;
            // use dummy base URL string because the URL constructor only accepts absolute URLs.
            const localVarUrlObj = new URL(localVarPath, 'https://example.com');
            let baseOptions;
            if (configuration) {
                baseOptions = configuration.baseOptions;
            }
            const localVarRequestOptions :AxiosRequestConfig = { method: 'POST', ...baseOptions, ...options};
            const localVarHeaderParameter = {} as any;
            const localVarQueryParameter = {} as any;

            // authentication Bearer required

            localVarHeaderParameter['Content-Type'] = 'application/json-patch+json';

            const query = new URLSearchParams(localVarUrlObj.search);
            for (const key in localVarQueryParameter) {
                query.set(key, localVarQueryParameter[key]);
            }
            for (const key in options.params) {
                query.set(key, options.params[key]);
            }
            localVarUrlObj.search = (new URLSearchParams(query)).toString();
            let headersFromBaseOptions = baseOptions && baseOptions.headers ? baseOptions.headers : {};
            localVarRequestOptions.headers = {...localVarHeaderParameter, ...headersFromBaseOptions, ...options.headers};
            const needsSerialization = (typeof body !== "string") || localVarRequestOptions.headers['Content-Type'] === 'application/json';
            localVarRequestOptions.data =  needsSerialization ? JSON.stringify(body !== undefined ? body : {}) : (body || "");

            return {
                url: localVarUrlObj.pathname + localVarUrlObj.search + localVarUrlObj.hash,
                options: localVarRequestOptions,
            };
        },
        /**
         * 
         * @summary 获取所有作业任务列表
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        sysJobListGet: async (options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            const localVarPath = `/sysJob/list`;
            // use dummy base URL string because the URL constructor only accepts absolute URLs.
            const localVarUrlObj = new URL(localVarPath, 'https://example.com');
            let baseOptions;
            if (configuration) {
                baseOptions = configuration.baseOptions;
            }
            const localVarRequestOptions :AxiosRequestConfig = { method: 'GET', ...baseOptions, ...options};
            const localVarHeaderParameter = {} as any;
            const localVarQueryParameter = {} as any;

            // authentication Bearer required

            const query = new URLSearchParams(localVarUrlObj.search);
            for (const key in localVarQueryParameter) {
                query.set(key, localVarQueryParameter[key]);
            }
            for (const key in options.params) {
                query.set(key, options.params[key]);
            }
            localVarUrlObj.search = (new URLSearchParams(query)).toString();
            let headersFromBaseOptions = baseOptions && baseOptions.headers ? baseOptions.headers : {};
            localVarRequestOptions.headers = {...localVarHeaderParameter, ...headersFromBaseOptions, ...options.headers};

            return {
                url: localVarUrlObj.pathname + localVarUrlObj.search + localVarUrlObj.hash,
                options: localVarRequestOptions,
            };
        },
    }
};

/**
 * SysJobApi - functional programming interface
 * @export
 */
export const SysJobApiFp = function(configuration?: Configuration) {
    return {
        /**
         * 
         * @summary 增加作业任务
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async sysJobAddPost(options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<AdminResultScheduleResult>>> {
            const localVarAxiosArgs = await SysJobApiAxiosParamCreator(configuration).sysJobAddPost(options);
            return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
                const axiosRequestArgs :AxiosRequestConfig = {...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url};
                return axios.request(axiosRequestArgs);
            };
        },
        /**
         * 
         * @summary 删除作业任务
         * @param {JobInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async sysJobDeletePost(body?: JobInput, options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<AdminResultScheduleResult>>> {
            const localVarAxiosArgs = await SysJobApiAxiosParamCreator(configuration).sysJobDeletePost(body, options);
            return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
                const axiosRequestArgs :AxiosRequestConfig = {...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url};
                return axios.request(axiosRequestArgs);
            };
        },
        /**
         * 
         * @summary 获取所有作业任务列表
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async sysJobListGet(options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<AdminResultListSchedulerModel>>> {
            const localVarAxiosArgs = await SysJobApiAxiosParamCreator(configuration).sysJobListGet(options);
            return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
                const axiosRequestArgs :AxiosRequestConfig = {...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url};
                return axios.request(axiosRequestArgs);
            };
        },
    }
};

/**
 * SysJobApi - factory interface
 * @export
 */
export const SysJobApiFactory = function (configuration?: Configuration, basePath?: string, axios?: AxiosInstance) {
    return {
        /**
         * 
         * @summary 增加作业任务
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async sysJobAddPost(options?: AxiosRequestConfig): Promise<AxiosResponse<AdminResultScheduleResult>> {
            return SysJobApiFp(configuration).sysJobAddPost(options).then((request) => request(axios, basePath));
        },
        /**
         * 
         * @summary 删除作业任务
         * @param {JobInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async sysJobDeletePost(body?: JobInput, options?: AxiosRequestConfig): Promise<AxiosResponse<AdminResultScheduleResult>> {
            return SysJobApiFp(configuration).sysJobDeletePost(body, options).then((request) => request(axios, basePath));
        },
        /**
         * 
         * @summary 获取所有作业任务列表
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async sysJobListGet(options?: AxiosRequestConfig): Promise<AxiosResponse<AdminResultListSchedulerModel>> {
            return SysJobApiFp(configuration).sysJobListGet(options).then((request) => request(axios, basePath));
        },
    };
};

/**
 * SysJobApi - object-oriented interface
 * @export
 * @class SysJobApi
 * @extends {BaseAPI}
 */
export class SysJobApi extends BaseAPI {
    /**
     * 
     * @summary 增加作业任务
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof SysJobApi
     */
    public async sysJobAddPost(options?: AxiosRequestConfig) : Promise<AxiosResponse<AdminResultScheduleResult>> {
        return SysJobApiFp(this.configuration).sysJobAddPost(options).then((request) => request(this.axios, this.basePath));
    }
    /**
     * 
     * @summary 删除作业任务
     * @param {JobInput} [body] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof SysJobApi
     */
    public async sysJobDeletePost(body?: JobInput, options?: AxiosRequestConfig) : Promise<AxiosResponse<AdminResultScheduleResult>> {
        return SysJobApiFp(this.configuration).sysJobDeletePost(body, options).then((request) => request(this.axios, this.basePath));
    }
    /**
     * 
     * @summary 获取所有作业任务列表
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof SysJobApi
     */
    public async sysJobListGet(options?: AxiosRequestConfig) : Promise<AxiosResponse<AdminResultListSchedulerModel>> {
        return SysJobApiFp(this.configuration).sysJobListGet(options).then((request) => request(this.axios, this.basePath));
    }
}