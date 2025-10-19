'use server';

import { Page } from "@/components/pages/page/Page";

export default async function pageContainer(
	{ params, searchParams }: {
		params: Promise<{ workspaceId: string, pageId: string }>,
		searchParams: Promise<{ blockId?: string }>
	}) {

	const { pageId } = await params;
	const { blockId } = await searchParams;

	return <Page pageId={pageId} blockId={blockId}/>
}