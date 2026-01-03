'use server';

import { Page } from "@/components/pages/page/Page";

export default async function pageContainer(
	{ params, searchParams }: {
		params: { workspaceId: string, pageId: string },
		searchParams?: { blockId?: string }
	}) {

	const { pageId } = params;
	const blockId = searchParams?.blockId;

	return <Page pageId={pageId} blockId={blockId}/>
}